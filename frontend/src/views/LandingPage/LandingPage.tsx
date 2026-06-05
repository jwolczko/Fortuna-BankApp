import { useState } from 'react';
import { LandingHeader } from '../../components/landing/LandingHeader/LandingHeader';
import { LandingHero } from '../../components/landing/LandingHero/LandingHero';
import { LoginPanel } from '../../components/login/LoginPanel/LoginPanel';
import { LoginSupportModal } from '../../components/login/LoginSupportModal/LoginSupportModal';
import './LandingPage.css';

export function LandingPage() {
  const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);
  const [isSupportModalOpen, setIsSupportModalOpen] = useState(false);

  return (
    <div className="landing-page">
      <LandingHeader onOpenLogin={() => setIsLoginModalOpen(true)} />
      <LandingHero />

      {isLoginModalOpen && (
        <div
          className="landing-page__login-overlay"
          role="dialog"
          aria-modal="true"
          aria-label="Panel logowania"
          onClick={() => setIsLoginModalOpen(false)}
        >
          <div className="landing-page__login-dialog" onClick={(event) => event.stopPropagation()}>
            <LoginPanel
              isModal
              onClose={() => setIsLoginModalOpen(false)}
              onOpenSupport={() => {
                setIsLoginModalOpen(false);
                setIsSupportModalOpen(true);
              }}
            />
          </div>
        </div>
      )}

      {isSupportModalOpen && <LoginSupportModal onClose={() => setIsSupportModalOpen(false)} />}
    </div>
  );
}
