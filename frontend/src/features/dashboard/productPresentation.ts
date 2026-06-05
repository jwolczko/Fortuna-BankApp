import type { DashboardProduct } from './types/dashboard.types';

const categoryLabels: Record<string, string> = {
  BankAccount: 'Konto',
  Card: 'Karta',
  Loan: 'Kredyt',
};

const productNameLabels: Record<string, string> = {
  'Standard Account': 'Konto Standardowe',
  'Prestige Account': 'Konto Prestige',
  'Debit Card': 'Karta Debetowa',
  'Credit Card': 'Karta Kredytowa',
};

const typeLabels: Record<string, string> = {
  Standard: 'Standardowe',
  Prestige: 'Prestige',
  Savings: 'Oszczędnościowe',
  Debit: 'Debetowa',
  Credit: 'Kredytowa',
  Cash: 'Gotówkowy',
  Consumer: 'Konsumpcyjny',
};

export function getProductCategoryLabel(category: string) {
  return categoryLabels[category] ?? category;
}

export function getProductTypeLabel(type: string) {
  return typeLabels[type] ?? type;
}

export function getProductDisplayName(name: string) {
  return productNameLabels[name] ?? name;
}

function groupDigits(value: string, groupSizes: number[]) {
  let currentIndex = 0;

  return groupSizes
    .map((groupSize) => {
      const chunk = value.slice(currentIndex, currentIndex + groupSize);
      currentIndex += groupSize;
      return chunk;
    })
    .filter(Boolean)
    .join(' ');
}

export function formatProductNumber(product: DashboardProduct) {
  const normalizedNumber = product.productNumber.replace(/\s+/g, '');

  if (product.productCategory === 'BankAccount' && normalizedNumber.length === 26) {
    return groupDigits(normalizedNumber, [2, 4, 4, 4, 4, 4, 4]);
  }

  if (product.productCategory === 'Card' && normalizedNumber.length === 16) {
    return groupDigits(normalizedNumber, [4, 4, 4, 4]);
  }

  return product.productNumber;
}

export function getProductSubtitle(product: DashboardProduct) {
  if (product.productCategory === 'BankAccount') {
    return formatProductNumber(product);
  }

  if (product.productCategory === 'Card') {
    return formatProductNumber(product);
  }

  return `${getProductCategoryLabel(product.productCategory)} ${getProductTypeLabel(product.productType)}`;
}

export function getProductAmountLabel(product: DashboardProduct) {
  if (product.productCategory === 'Card' && product.productType === 'Credit') {
    return 'Dostępny limit';
  }

  if (product.productCategory === 'Loan') {
    return 'Pozostało do spłaty';
  }

  return 'Dostępne środki';
}

export function getProductDisplayBalance(product: DashboardProduct, products: DashboardProduct[]) {
  if (product.productCategory === 'Card' && product.productType === 'Debit') {
    const linkedAccount =
      products.find(
        (candidate) =>
          candidate.productCategory === 'BankAccount'
          && (candidate.productType === 'Standard' || candidate.productType === 'Prestige'),
      ) ?? products.find((candidate) => candidate.productCategory === 'BankAccount');

    if (linkedAccount) {
      return {
        amount: linkedAccount.balance,
        currency: linkedAccount.currency,
      };
    }
  }

  return {
    amount: product.balance,
    currency: product.currency,
  };
}
