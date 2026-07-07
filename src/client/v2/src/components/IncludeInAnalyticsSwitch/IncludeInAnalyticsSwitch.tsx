import { ChangeEvent } from 'react';
import { useMutation } from '@apollo/client/react';
import { Switch } from '@mantine/core';
import { SetBankAccountIncludeInAnalytics } from '@graphql-mutations/SetBankAccountIncludeInAnalytics';

interface IncludeInAnalyticsSwitchProps {
  /** The id of the bank account whose inclusion state is being controlled. */
  accountId: string;
  /** Whether the account is currently included in analytics. */
  includeInAnalytics: boolean;
  /** Size of the rendered switch. */
  size?: string;
}

/**
 * Switch that toggles whether a bank account is included in analytics such as graphs and account
 * views. Persists the change through the {@link SetBankAccountIncludeInAnalytics} mutation and
 * relies on Apollo's normalized cache (keyed by account id) to keep every view in sync.
 *
 * The `checked` state is derived from the passed in prop rather than local state so that the switch
 * always reflects the cached value. An optimistic response makes the toggle feel instant and
 * automatically reverts if the mutation fails.
 */
export function IncludeInAnalyticsSwitch({
  accountId,
  includeInAnalytics,
  size = 'sm',
}: IncludeInAnalyticsSwitchProps) {
  const [setIncludeInAnalytics] = useMutation(SetBankAccountIncludeInAnalytics);

  const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
    const nextValue = event.currentTarget.checked;

    void setIncludeInAnalytics({
      variables: { bankAccountId: accountId, includeInAnalytics: nextValue },
      optimisticResponse: {
        setBankAccountIncludeInAnalytics: {
          __typename: 'SetBankAccountIncludeInAnalyticsPayload',
          bankAccount: {
            __typename: 'BankAccount',
            id: accountId,
            includeInAnalytics: nextValue,
          },
        },
      },
    });
  };

  return (
    <Switch
      size={size}
      checked={includeInAnalytics}
      onChange={handleChange}
      label="Include in analytics"
      styles={{ label: { paddingInlineStart: 'var(--mantine-spacing-xs)' } }}
    />
  );
}
