import type { ButtonHTMLAttributes, PropsWithChildren } from 'react';
import './Button.css';

type ButtonVariant = 'primary' | 'outline' | 'ghost';

type ButtonProps = PropsWithChildren<ButtonHTMLAttributes<HTMLButtonElement>> & {
  variant?: ButtonVariant;
};

export function Button({ children, variant = 'primary', className = '', ...props }: ButtonProps) {
  const appVariant = variant === 'primary' ? ' app-button--primary' : variant === 'ghost' ? ' app-button--success' : '';

  return (
    <button className={`app-button${appVariant} ui-button ui-button--${variant} ${className}`.trim()} {...props}>
      {children}
    </button>
  );
}
