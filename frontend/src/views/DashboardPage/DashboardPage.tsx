import { useState } from 'react';
import { Navigate } from 'react-router-dom';
import { useAppSelector } from '../../app/store/hooks';
import { DashboardHeader } from '../../components/dashboard/DashboardHeader/DashboardHeader';
import { useDashboard } from '../../features/dashboard/hooks/useDashboard';
import { SummarySection } from '../../components/dashboard/SummarySection/SummarySection';
import { ProductsSection } from '../../components/dashboard/ProductsSection/ProductsSection';
import { EventsSidebar } from '../../components/dashboard/EventsSidebar/EventsSidebar';
import { TransferPanel } from '../../components/transfers/TransferPanel/TransferPanel';
import './DashboardPage.css';

export function DashboardPage() {
  const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
  const [isTransferPanelOpen, setIsTransferPanelOpen] = useState(false);
  const { data, isLoading, isError, error } = useDashboard();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (isLoading) {
    return <div className="dashboard-page">Ladowanie danych dashboardu...</div>;
  }

  if (isError || !data) {
    return <div className="dashboard-page">{error instanceof Error ? error.message : 'Nie udalo sie pobrac dashboardu.'}</div>;
  }

  return (
    <div className="dashboard-page">
      <DashboardHeader />

      <div className="dashboard-page__content">
        <main className="dashboard-page__main">
          <SummarySection dashboard={data} onOpenTransfer={() => setIsTransferPanelOpen(true)} />
          <ProductsSection dashboard={data} />
        </main>
        
        <aside className="dashboard-page__sidebar">
          <EventsSidebar dashboard={data} />
        </aside>
      </div>

      {isTransferPanelOpen && <TransferPanel dashboard={data} onClose={() => setIsTransferPanelOpen(false)} />}
    </div>
  );
}
