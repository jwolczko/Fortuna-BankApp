import './LoginSupportModal.css';

type LoginSupportModalProps = {
  onClose: () => void;
};

export function LoginSupportModal({ onClose }: LoginSupportModalProps) {
  return (
    <div className="login-support-modal__overlay" role="dialog" aria-modal="true">
      <div className="login-support-modal">
        <button className="app-button app-button--icon app-button--primary login-support-modal__close" type="button" onClick={onClose}>
          ×
        </button>

        <div className="login-support-modal__lock" />

        <div className="login-support-modal__top-line" />

        <div className="login-support-modal__form-row">
          <div className="login-support-modal__field">
            <div className="login-support-modal__field-header">
              <label>Login:</label>
              <button className="app-button login-support-modal__help-btn" type="button">Nie pamiętasz loginu?</button>
            </div>
            <input type="text" placeholder="Wpisz login lub adres e-mail" />
          </div>

          <button className="app-button app-button--primary login-support-modal__next" type="button">
            DALEJ
          </button>
        </div>

        <div className="login-support-modal__info-grid">
          <div className="login-support-modal__info-card login-support-modal__info-card--divider">
            <h3>⊖ Finanse</h3>
            <p>Sprawdź saldo i zlecaj przelewy z kont w innych bankach wygodnie przez Millenet</p>
            <a>Więcej</a>
          </div>

          <div className="login-support-modal__info-card">
            <h3>⊖ Fałszywi konsultanci</h3>
            <p>
              Oszuści podszywają się pod pracowników Banku - nie pobieraj nieznanych aplikacji i nie
              udostępniaj poufnych danych
            </p>
            <a>Więcej</a>
          </div>
        </div>
      </div>
    </div>
  );
}
