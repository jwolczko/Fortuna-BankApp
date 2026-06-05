import { type CSSProperties, useState } from 'react';
import { InfoPopup } from '../../../shared/ui/InfoPopup/InfoPopup';
import fortunaImage from '../../../assets/landing/fortuna-clean.png';
import './LandingHero.css';

export function LandingHero() {
  const [isInfoPopupOpen, setIsInfoPopupOpen] = useState(false);

  const handleOpenInfoPopup = () => {
    setIsInfoPopupOpen(true);
  };

  return (
    <>
      <section className="landing-hero">
        <div className="landing-hero__grid">
          <div className="landing-hero__content">
            <h1>Zyskaj do 400 zł premii za płatności kartą w telefonie</h1>
            <p>Zamów kartę kredytową Visa Impresja (RRSO 18,28%) i spełnij warunki promocji</p>

            <div className="landing-hero__actions">
              <button className="app-button app-button--primary landing-hero__primary-btn" type="button" onClick={handleOpenInfoPopup}>
                ZŁÓŻ WNIOSEK
              </button>
              <button className="app-button landing-hero__secondary-btn" type="button" onClick={handleOpenInfoPopup}>
                DOWIEDZ SIĘ WIĘCEJ
              </button>
              <span className="landing-hero__legal-link">Koszt kredytu i nota prawna</span>
            </div>

            <button className="app-button app-button--icon landing-hero__play-btn" type="button" aria-label="Odtwórz materiał promocyjny">
              <span>▶</span>
            </button>
          </div>

          <div
            className="landing-hero__visual"
            style={{ '--fortuna-landing-image': `url(${fortunaImage})` } as CSSProperties}
            role="img"
            aria-label="Fortuna z opaską na oczach, kołem losu i rogiem obfitości"
          />
        </div>

        <div className="landing-hero__cards">
          <div className="landing-hero__card landing-hero__card--dark">
            <strong>KARTA KREDYTOWA VISA IMPRESJA (RRSO 18,28%)</strong>
            <p>Promocja do 31.08.2026</p>
            <span>›</span>
          </div>
          <div className="landing-hero__card">
            <strong>POŻYCZKA GOTÓWKOWA (RRSO 8,3%)</strong>
            <p>Promocja do 15.05.2026</p>
          </div>
          <div className="landing-hero__card">
            <strong>KREDYT HIPOTECZNY (RRSO 6,49%)</strong>
            <p>Znajdź swoje miejsce</p>
          </div>
          <div className="landing-hero__card">
            <strong>ZWROTY ZA ZAKUPY</strong>
            <p>Przemyślany sposób na zakupy</p>
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
