export type DashboardProduct = {
  productId: string;
  productCategory: string;
  productType: string;
  productName: string;
  productNumber: string;
  balance: number;
  currency: string;
};

export type DashboardEvent = {
  id: string;
  eventDateUtc: string;
  eventType: string;
  title: string;
  amount: number;
  currency: string;
  isPositive: boolean;
};

export type DashboardData = {
  customerId: string;
  totalBalance: number;
  currency: string;
  products: DashboardProduct[];
  events: DashboardEvent[];
};
