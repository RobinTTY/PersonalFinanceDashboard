import React from 'react';

export interface AccountSummaryProps {
  description: string;
  balance: number;
  currency: string;
  icon?: React.ReactElement;
}
