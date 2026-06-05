import type { InputHTMLAttributes } from 'react';
import { forwardRef } from 'react';
import './Input.css';

type InputProps = InputHTMLAttributes<HTMLInputElement> & {
  label?: string;
  error?: string;
};

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, id, error, className = '', ...props }, ref) => {
    return (
      <div className="ui-input-wrapper">
        {label && (
          <label className="ui-input-label" htmlFor={id}>
            {label}
          </label>
        )}

        <input
          ref={ref}
          id={id}
          className={`ui-input ${error ? 'ui-input--error' : ''} ${className}`}
          {...props}
        />

        {error && <span className="ui-input-error">{error}</span>}
      </div>
    );
  }
);

Input.displayName = 'Input';
