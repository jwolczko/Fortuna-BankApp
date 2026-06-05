import { useState } from 'react';
import type { DashboardData } from '../../../features/dashboard/types/dashboard.types';
import { getProductDisplayName } from '../../../features/dashboard/productPresentation';
import { InfoPopup } from '../../../shared/ui/InfoPopup/InfoPopup';
import './SummarySection.css';

type SummarySectionProps = {
  dashboard: DashboardData;
  onOpenTransfer: () => void;
};

function formatMoney(amount: number, currency: string) {
  return new Intl.NumberFormat('pl-PL', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount) + ` ${currency}`;
}

export function SummarySection({ dashboard, onOpenTransfer }: SummarySectionProps) {
  const [isInfoPopupOpen, setIsInfoPopupOpen] = useState(false);
  const featuredProduct = dashboard.products[0];

  return (
    <>
      <section className="summary-section">
        <div className="summary-section__header-row">
          <h2>Podsumowanie środków</h2>
        </div>

        <div className="summary-section__top-grid">
          <div className="summary-section__left">
            <div className="summary-section__stack summary-section__stack--back-1" />
            <div className="summary-section__stack summary-section__stack--back-2" />

            <div className="summary-section__card">
              <div className="summary-section__card-header">
                <h3>{featuredProduct ? getProductDisplayName(featuredProduct.productName) : 'Brak aktywnych produktow'}</h3>
              </div>

              <div className="summary-section__amount">
                {formatMoney(dashboard.totalBalance, dashboard.currency)}
              </div>

              <div className="summary-section__actions">
                <button type="button" className="app-button app-button--primary summary-section__primary-btn" onClick={onOpenTransfer}>
                  WYKONAJ PRZELEW
                </button>
                <button
                  type="button"
                  className="app-button summary-section__outline-btn"
                  onClick={() => setIsInfoPopupOpen(true)}
                >
                  HISTORIA
                </button>
              </div>
            </div>
          </div>

          
          <div className="summary-section__right">         
          </div>
        </div>
      </section>

      {isInfoPopupOpen && (
        <InfoPopup
          message="Funkcjonalność nie jest jeszcze zaimplementowana"
          onClose={() => setIsInfoPopupOpen(false)}
        />
      )}
    </>
  );
}
