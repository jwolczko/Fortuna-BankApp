import type { FormEvent } from 'react';
import { useEffect, useMemo, useState } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { useAppSelector } from '../../../app/store/hooks';
import { formatProductNumber, getProductDisplayName } from '../../../features/dashboard/productPresentation';
import type { DashboardData } from '../../../features/dashboard/types/dashboard.types';
import { createTransferRequest } from '../../../features/transfers/api/transferApi';
import { updateDashboardAfterTransfer } from '../../../features/transfers/dashboardTransferUpdater';
import type { TransferMode } from '../../../features/transfers/types/transfer.types';
import './TransferPanel.css';

type TransferPanelProps = {
  dashboard: DashboardData;
  onClose: () => void;
};

export function TransferPanel({ dashboard, onClose }: TransferPanelProps) {
  const queryClient = useQueryClient();
  const token = useAppSelector((state) => state.auth.token);
  const customerId = useAppSelector((state) => state.auth.customerId);
  const bankAccounts = useMemo(
    () => dashboard.products.filter((product) => product.productCategory === 'BankAccount'),
    [dashboard.products],
  );

  const [transferType, setTransferType] = useState<TransferMode>(bankAccounts.length > 1 ? 'Own' : 'External');
  const [sourceAccountId, setSourceAccountId] = useState(bankAccounts[0]?.productId ?? '');
  const [targetAccountId, setTargetAccountId] = useState(bankAccounts[1]?.productId ?? bankAccounts[0]?.productId ?? '');
  const [targetAccountNumber, setTargetAccountNumber] = useState('');
  const [recipientName, setRecipientName] = useState('');
  const [title, setTitle] = useState('');
  const [amount, setAmount] = useState('');
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (bankAccounts.length <= 1) {
      setTransferType('External');
    }
  }, [bankAccounts.length]);

  useEffect(() => {
    if (!bankAccounts.some((account) => account.productId === sourceAccountId)) {
      setSourceAccountId(bankAccounts[0]?.productId ?? '');
    }
  }, [bankAccounts, sourceAccountId]);

  useEffect(() => {
    if (transferType !== 'Own') {
      return;
    }

    const availableTargetAccounts = bankAccounts.filter((account) => account.productId !== sourceAccountId);

    if (!availableTargetAccounts.some((account) => account.productId === targetAccountId)) {
      setTargetAccountId(availableTargetAccounts[0]?.productId ?? '');
    }
  }, [bankAccounts, sourceAccountId, targetAccountId, transferType]);

  const availableTargetAccounts = bankAccounts.filter((account) => account.productId !== sourceAccountId);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!token || !customerId || !sourceAccountId || !title.trim() || !amount.trim() || isSubmitting) {
      return;
    }

    const parsedAmount = Number(amount);

    if (!Number.isFinite(parsedAmount) || parsedAmount <= 0) {
      setErrorMessage('Kwota przelewu musi być większa od zera.');
      return;
    }

    if (transferType === 'Own' && !targetAccountId) {
      setErrorMessage('Wybierz konto docelowe.');
      return;
    }

    if (transferType === 'External' && (!targetAccountNumber.trim() || !recipientName.trim())) {
      setErrorMessage('Uzupełnij numer konta docelowego i nazwę odbiorcy.');
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage(null);

      const payload = {
        transferType,
        sourceAccountId,
        targetAccountId: transferType === 'Own' ? targetAccountId : undefined,
        targetAccountNumber: transferType === 'External' ? targetAccountNumber.trim() : undefined,
        recipientName: transferType === 'External' ? recipientName.trim() : undefined,
        amount: parsedAmount,
        currency: 'PLN',
        title: title.trim(),
      } as const;

      await createTransferRequest(token, payload);
      updateDashboardAfterTransfer(queryClient, customerId, payload);
      onClose();
      window.setTimeout(() => {
        void queryClient.invalidateQueries({ queryKey: ['dashboard', customerId] });
        void queryClient.refetchQueries({ queryKey: ['dashboard', customerId] });
      }, 2500);
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : 'Nie udało się wykonać przelewu.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="transfer-panel__overlay" role="dialog" aria-modal="true" aria-label="Panel przelewów" onClick={onClose}>
      <div className="transfer-panel" onClick={(event) => event.stopPropagation()}>
        <form className="transfer-panel__form" onSubmit={handleSubmit}>
          <h2>Do przelewów</h2>

          <label className="transfer-panel__label" htmlFor="transferType">
            Typ przelewu
          </label>
          <select
            id="transferType"
            className="transfer-panel__input"
            value={transferType}
            onChange={(event) => setTransferType(event.target.value as TransferMode)}
            disabled={isSubmitting}
          >
            {bankAccounts.length > 1 && <option value="Own">Własny</option>}
            <option value="External">Zewnętrzny</option>
          </select>

          {bankAccounts.length > 1 ? (
            <>
              <label className="transfer-panel__label" htmlFor="sourceAccountId">
                Konto źródłowe
              </label>
              <select
                id="sourceAccountId"
                className="transfer-panel__input"
                value={sourceAccountId}
                onChange={(event) => setSourceAccountId(event.target.value)}
                disabled={isSubmitting}
              >
                {bankAccounts.map((account) => (
                  <option key={account.productId} value={account.productId}>
                    {getProductDisplayName(account.productName)} • {formatProductNumber(account)}
                  </option>
                ))}
              </select>
            </>
          ) : (
            <>
              <label className="transfer-panel__label">Konto źródłowe</label>
              <div className="transfer-panel__readonly">
                {bankAccounts[0] ? `${getProductDisplayName(bankAccounts[0].productName)} • ${formatProductNumber(bankAccounts[0])}` : 'Brak kont do przelewu'}
              </div>
            </>
          )}

          {transferType === 'Own' ? (
            <>
              <label className="transfer-panel__label" htmlFor="targetAccountId">
                Konto docelowe
              </label>
              <select
                id="targetAccountId"
                className="transfer-panel__input"
                value={targetAccountId}
                onChange={(event) => setTargetAccountId(event.target.value)}
                disabled={isSubmitting}
              >
                {availableTargetAccounts.map((account) => (
                  <option key={account.productId} value={account.productId}>
                    {getProductDisplayName(account.productName)} • {formatProductNumber(account)}
                  </option>
                ))}
              </select>
            </>
          ) : (
            <>
              <label className="transfer-panel__label" htmlFor="targetAccountNumber">
                Konto docelowe
              </label>
              <input
                id="targetAccountNumber"
                className="transfer-panel__input"
                type="text"
                value={targetAccountNumber}
                placeholder="Wpisz numer konta"
                inputMode="numeric"
                pattern="[0-9]*"
                onChange={(event) => setTargetAccountNumber(event.target.value.replace(/\D+/g, ''))}
                disabled={isSubmitting}
              />

              <label className="transfer-panel__label" htmlFor="recipientName">
                Odbiorca
              </label>
              <input
                id="recipientName"
                className="transfer-panel__input"
                type="text"
                value={recipientName}
                placeholder="Wpisz nazwę odbiorcy"
                onChange={(event) => setRecipientName(event.target.value)}
                disabled={isSubmitting}
              />
            </>
          )}

          <label className="transfer-panel__label" htmlFor="title">
            Tytuł przelewu
          </label>
          <input
            id="title"
            className="transfer-panel__input"
            type="text"
            value={title}
            placeholder="np. Przelew środków"
            onChange={(event) => setTitle(event.target.value)}
            disabled={isSubmitting}
          />

          <label className="transfer-panel__label" htmlFor="amount">
            Kwota
          </label>
          <input
            id="amount"
            className="transfer-panel__input"
            type="number"
            min="0.01"
            step="0.01"
            value={amount}
            placeholder="0,00"
            onChange={(event) => setAmount(event.target.value)}
            disabled={isSubmitting}
          />

          {errorMessage && <p className="transfer-panel__error">{errorMessage}</p>}

          <div className="transfer-panel__buttons">
            <button className="app-button transfer-panel__secondary-btn" type="button" onClick={onClose}>
              ANULUJ
            </button>
            <button className="app-button app-button--primary transfer-panel__primary-btn" type="submit" disabled={isSubmitting || bankAccounts.length === 0}>
              {isSubmitting ? 'WYSYŁANIE...' : 'WYKONAJ PRZELEW'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
