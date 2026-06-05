import { useState } from 'react';
import type { DashboardData } from '../../../features/dashboard/types/dashboard.types';
import {
  getProductAmountLabel,
  getProductCategoryLabel,
  getProductDisplayBalance,
  getProductDisplayName,
  getProductSubtitle,
  getProductTypeLabel,
} from '../../../features/dashboard/productPresentation';
import { InfoPopup } from '../../../shared/ui/InfoPopup/InfoPopup';
import './ProductsSection.css';

type ProductsSectionProps = {
  dashboard: DashboardData;
};

function formatMoney(amount: number, currency: string) {
  return new Intl.NumberFormat('pl-PL', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount) + ` ${currency}`;
}

export function ProductsSection({ dashboard }: ProductsSectionProps) {
  const [isInfoPopupOpen, setIsInfoPopupOpen] = useState(false);

  return (
    <>
      <section className="products-section">
        <div className="products-section__header">
          <h2>Moje produkty</h2>
        </div>

        <div className="products-section__grid">
          {dashboard.products.map((product) => {
            const displayBalance = getProductDisplayBalance(product, dashboard.products);

            return (
              <article className="products-section__card" key={product.productId}>
                <div className="products-section__card-header">
                  <h3>{getProductDisplayName(product.productName)}</h3>
                  <button className="app-button app-button--icon" type="button">⋮</button>
                </div>

                <div className="products-section__subtitle">{getProductSubtitle(product)}</div>
                <div className="products-section__label">
                  {getProductCategoryLabel(product.productCategory)} • {getProductTypeLabel(product.productType)}
                </div>
                <div className="products-section__label">{getProductAmountLabel(product)}</div>
                <div className="products-section__amount">{formatMoney(displayBalance.amount, displayBalance.currency)}</div>
              </article>
            );
          })}

          <article
            className="app-button app-button--success products-section__add-card"
            role="button"
            tabIndex={0}
            onClick={() => setIsInfoPopupOpen(true)}
            onKeyDown={(event) => {
              if (event.key === 'Enter' || event.key === ' ') {
                event.preventDefault();
                setIsInfoPopupOpen(true);
              }
            }}
          >
            <div className="products-section__plus">＋</div>
            <h3>Dodaj nowy produkt</h3>
            <p>do swojej bankowości</p>
          </article>
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
