import type { DashboardData } from '../../../features/dashboard/types/dashboard.types';
import './EventsSidebar.css';

type EventsSidebarProps = {
  dashboard: DashboardData;
};

function formatMoney(amount: number, currency: string) {
  return new Intl.NumberFormat('pl-PL', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount) + ` ${currency}`;
}

function formatEventDate(eventDateUtc: string) {
  return new Intl.DateTimeFormat('pl-PL', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
  }).format(new Date(eventDateUtc));
}

export function EventsSidebar({ dashboard }: EventsSidebarProps) {
  return (
    <section className="events-sidebar">
      <h2>Wydarzenia</h2>

      {dashboard.events.map((event) => (
        <div className="events-sidebar__group" key={event.id}>
          <h3>{formatEventDate(event.eventDateUtc)}</h3>
          <div className="events-sidebar__items">
            <article
              className={`events-sidebar__item ${event.eventType === 'transfer' ? 'events-sidebar__item--success' : ''}`}
            >
                <div className="events-sidebar__item-left">
                  <span className={`events-sidebar__icon ${event.isPositive ? 'events-sidebar__icon--positive' : ''}`}>
                    {event.isPositive ? '↪' : '⊞'}
                  </span>
                  <span className="events-sidebar__text">{event.title}</span>
                </div>
                <div className={`events-sidebar__amount ${event.isPositive ? 'events-sidebar__amount--positive' : ''}`}>
                    {formatMoney(event.amount, event.currency)} <span>▾</span>
                  </div>
              </article>
          </div>
        </div>
      ))}
    </section>
  );
}
