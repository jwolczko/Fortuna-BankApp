import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { useAppDispatch } from '../../../app/store/hooks';
import { loginAndInitializeSession } from '../../../features/auth/authFlow';
import './LoginPanel.css';

type LoginPanelProps = {
  onOpenSupport: () => void;
  onClose?: () => void;
  isModal?: boolean;
};

export function LoginPanel({ onOpenSupport, onClose, isModal = false }: LoginPanelProps) {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const queryClient = useQueryClient();
  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (!login.trim() || !password.trim() || isSubmitting) {
      return;
    }

    try {
      setIsSubmitting(true);
      setErrorMessage(null);

      await loginAndInitializeSession(login.trim(), password, dispatch, queryClient);

      onClose?.();
      navigate('/dashboard');
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : 'Logowanie nie powiodło się.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleFormSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await handleSubmit();
  };

  return (
    <div className={`login-panel${isModal ? ' login-panel--modal' : ''}`}>
      <form className="login-panel__form" onSubmit={handleFormSubmit}>
          <h1>Logowanie do systemu:</h1>

          <label className="login-panel__label" htmlFor="login">
            Twój login
          </label>
          <input
            id="login"
            className="login-panel__input"
            type="email"
            value={login}
            placeholder="np. anna.kowalska@example.com"
            onChange={(event) => setLogin(event.target.value)}
            autoComplete="username"
            disabled={isSubmitting}
          />

          <label className="login-panel__label" htmlFor="password">
            Hasło <span className="login-panel__hint"></span>
          </label>
          <input
            id="password"
            className="login-panel__input"
            type="password"
            value={password}
            placeholder="Wpisz hasło"
            onChange={(event) => setPassword(event.target.value)}
            autoComplete="current-password"
            disabled={isSubmitting}
          />

          {errorMessage && <p className="login-panel__error">{errorMessage}</p>}

          <div className="login-panel__buttons">
            {onClose ? (
              <button className="app-button login-panel__back-btn" type="button" onClick={onClose}>
                WSTECZ
              </button>
            ) : (
              <Link className="app-button login-panel__back-btn" to="/">
                WSTECZ
              </Link>
            )}
            <button className="app-button app-button--primary login-panel__submit-btn" type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'LOGOWANIE...' : 'ZALOGUJ'}
            </button>
          </div>

          <button className="app-button login-panel__support-link" type="button" onClick={onOpenSupport}>
            Problemy z logowaniem
          </button>
      </form>
    </div>
  );
}
