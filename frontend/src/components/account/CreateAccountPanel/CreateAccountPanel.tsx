import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { useAppDispatch } from '../../../app/store/hooks';
import { registerRequest } from '../../../features/auth/api/authApi';
import { loginAndInitializeSession } from '../../../features/auth/authFlow';
import './CreateAccountPanel.css';

type CreateAccountPanelProps = {
  onClose?: () => void;
  isModal?: boolean;
};

export function CreateAccountPanel({ onClose, isModal = false }: CreateAccountPanelProps) {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const queryClient = useQueryClient();
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [customerType, setCustomerType] = useState<'Normal' | 'Prestige'>('Normal');
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (!firstName.trim() || !lastName.trim() || !email.trim() || !password.trim() || isSubmitting) {
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage(null);

      await registerRequest({
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        password,
        customerType,
      });

      await loginAndInitializeSession(email.trim(), password, dispatch, queryClient);

      onClose?.();
      navigate('/dashboard');
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : 'Nie udało się założyć konta.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className={`create-account-panel${isModal ? ' create-account-panel--modal' : ''}`}>
      <div className="create-account-panel__form">
        <h1>Załóż konto w Fortuna</h1>

        <label className="create-account-panel__label" htmlFor="firstName">
          Imię
        </label>
        <input
          id="firstName"
          className="create-account-panel__input"
          type="text"
          value={firstName}
          placeholder="np. Anna"
          onChange={(event) => setFirstName(event.target.value)}
          disabled={isSubmitting}
        />

        <label className="create-account-panel__label" htmlFor="lastName">
          Nazwisko
        </label>
        <input
          id="lastName"
          className="create-account-panel__input"
          type="text"
          value={lastName}
          placeholder="np. Kowalska"
          onChange={(event) => setLastName(event.target.value)}
          disabled={isSubmitting}
        />

        <label className="create-account-panel__label" htmlFor="email">
          Adres e-mail
        </label>
        <input
          id="email"
          className="create-account-panel__input"
          type="email"
          value={email}
          placeholder="np. anna.kowalska@example.com"
          onChange={(event) => setEmail(event.target.value)}
          autoComplete="username"
          disabled={isSubmitting}
        />

        <label className="create-account-panel__label" htmlFor="password">
          Hasło
        </label>
        <input
          id="password"
          className="create-account-panel__input"
          type="password"
          value={password}
          placeholder="Wpisz hasło"
          onChange={(event) => setPassword(event.target.value)}
          autoComplete="new-password"
          disabled={isSubmitting}
        />

        <label className="create-account-panel__label" htmlFor="customerType">
          Typ klienta
        </label>
        <select
          id="customerType"
          className="create-account-panel__input"
          value={customerType}
          onChange={(event) => setCustomerType(event.target.value as 'Normal' | 'Prestige')}
          disabled={isSubmitting}
        >
          <option value="Normal">Standard</option>
          <option value="Prestige">Prestige</option>
        </select>

        {errorMessage && <p className="create-account-panel__error">{errorMessage}</p>}

        <div className="create-account-panel__buttons">
          {onClose ? (
            <button className="app-button create-account-panel__back-btn" type="button" onClick={onClose}>
              WSTECZ
            </button>
          ) : (
            <Link className="app-button create-account-panel__back-btn" to="/">
              WSTECZ
            </Link>
          )}
          <button
            className="app-button app-button--primary create-account-panel__submit-btn"
            type="button"
            onClick={handleSubmit}
            disabled={isSubmitting}
          >
            {isSubmitting ? 'TWORZENIE...' : 'ZAŁÓŻ KONTO'}
          </button>
        </div>
      </div>
    </div>
  );
}
