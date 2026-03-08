import { axe, render } from '@test-utils';
import { screen, fireEvent } from '@testing-library/react';
import { AddAccountModal } from './AddAccountModal';

const noop = () => {};

describe('AddAccountModal', () => {
  axe([<AddAccountModal key="1" opened onClose={noop} />]);

  it('renders correctly when opened', () => {
    render(<AddAccountModal opened onClose={noop} />);
    expect(screen.getByRole('heading', { name: 'Choose Account Type' })).toBeInTheDocument();
  });

  it('does not render content when closed', () => {
    render(<AddAccountModal opened={false} onClose={noop} />);
    expect(screen.queryByText('Choose Account Type')).not.toBeInTheDocument();
  });

  it('shows both account type options', () => {
    render(<AddAccountModal opened onClose={noop} />);
    expect(screen.getByText('Savings Account')).toBeInTheDocument();
    expect(screen.getByText('Investment Account')).toBeInTheDocument();
  });

  it('shows the account type descriptions', () => {
    render(<AddAccountModal opened onClose={noop} />);
    expect(screen.getByText('Track your cash, checking, and savings balances.')).toBeInTheDocument();
    expect(screen.getByText('Track stocks, ETFs, and other investment portfolios.')).toBeInTheDocument();
  });

  it('has Next button disabled when no account type is selected', () => {
    render(<AddAccountModal opened onClose={noop} />);
    expect(screen.getByRole('button', { name: 'Next' })).toBeDisabled();
  });

  it('enables Next button after selecting an account type', () => {
    render(<AddAccountModal opened onClose={noop} />);
    fireEvent.click(screen.getByText('Savings Account'));
    expect(screen.getByRole('button', { name: 'Next' })).not.toBeDisabled();
  });

  it('enables Next button when Investment Account is selected', () => {
    render(<AddAccountModal opened onClose={noop} />);
    fireEvent.click(screen.getByText('Investment Account'));
    expect(screen.getByRole('button', { name: 'Next' })).not.toBeDisabled();
  });

  it('calls onClose when the close button is clicked', () => {
    const onClose = vi.fn();
    render(<AddAccountModal opened onClose={onClose} />);
    fireEvent.click(screen.getByRole('button', { name: 'Close' }));
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it('calls onClose when the Cancel button is clicked', () => {
    const onClose = vi.fn();
    render(<AddAccountModal opened onClose={onClose} />);
    fireEvent.click(screen.getByRole('button', { name: 'Cancel' }));
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it('resets selection when modal is closed and reopened', () => {
    const onClose = vi.fn();
    const { rerender } = render(<AddAccountModal opened onClose={onClose} />);
    fireEvent.click(screen.getByText('Savings Account'));
    expect(screen.getByRole('button', { name: 'Next' })).not.toBeDisabled();

    fireEvent.click(screen.getByRole('button', { name: 'Cancel' }));
    rerender(
      <AddAccountModal opened={false} onClose={onClose} />
    );
    rerender(
      <AddAccountModal opened onClose={onClose} />
    );
    expect(screen.getByRole('button', { name: 'Next' })).toBeDisabled();
  });
});
