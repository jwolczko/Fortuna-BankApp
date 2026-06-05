import { useNavigate } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { useAppDispatch, useAppSelector } from '../../../app/store/hooks';
import { clearAuthSession } from '../../../features/auth/authSession';
import { clearCredentials } from '../../../features/auth/store/authSlice';
import './DashboardHeader.css';

export function DashboardHeader() {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const queryClient = useQueryClient();
  const userName = useAppSelector((state) => state.auth.userName);

  const handleLogout = async () => {
    clearAuthSession();
    dispatch(clearCredentials());
    await queryClient.cancelQueries();
    queryClient.removeQueries({ queryKey: ['dashboard'] });
    navigate('/');
  };

  return (
    <header className="dashboard-header">
      <div className="dashboard-header__left">
        <span className="dashboard-header__page-name">Strona Główna</span>
      </div>

      <div className="dashboard-header__right">
        <div className="dashboard-header__user">
          <div className="dashboard-header__avatar">◯</div>
          <span>{userName ?? 'Klient Fortuna'}</span>
          
        </div>

        <div className="dashboard-header__mail">✉<span className="dashboard-header__badge">13</span></div>

        <button className="app-button dashboard-header__logout" type="button" onClick={handleLogout}>
          <span>⏻</span>
          <div>
            <strong>WYLOGUJ</strong>
            <span>05:00</span>
          </div>
        </button>
      </div>
    </header>
  );
}
