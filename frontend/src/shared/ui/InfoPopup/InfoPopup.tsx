import './InfoPopup.css';

type InfoPopupProps = {
  message: string;
  onClose: () => void;
};

export function InfoPopup({ message, onClose }: InfoPopupProps) {
  return (
    <div
      className="info-popup__overlay"
      role="dialog"
      aria-modal="true"
      aria-label="Informacja"
      onClick={onClose}
    >
      <div className="info-popup" onClick={(event) => event.stopPropagation()}>
        <p className="info-popup__message">{message}</p>
        <button className="app-button app-button--primary info-popup__button" type="button" onClick={onClose}>
          OK
        </button>
      </div>
    </div>
  );
}
