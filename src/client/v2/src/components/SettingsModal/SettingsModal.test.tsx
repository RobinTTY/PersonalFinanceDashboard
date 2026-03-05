import { axe, render } from '@test-utils';
import { screen, fireEvent } from '@testing-library/react';
import { SettingsModal } from './SettingsModal';

const noop = () => {};

describe('SettingsModal', () => {
  axe([<SettingsModal key="1" opened onClose={noop} />]);

  it('renders correctly when opened', () => {
    render(<SettingsModal opened onClose={noop} />);
  });

  it('does not render content when closed', () => {
    render(<SettingsModal opened={false} onClose={noop} />);
    expect(screen.queryByText('Preferences')).not.toBeInTheDocument();
  });

  it('shows Preferences as the default active section', () => {
    render(<SettingsModal opened onClose={noop} />);
    expect(screen.getByRole('heading', { name: 'Preferences' })).toBeInTheDocument();
  });

  it('shows all navigation section headings', () => {
    render(<SettingsModal opened onClose={noop} />);
    expect(screen.getByText('Account')).toBeInTheDocument();
    expect(screen.getByText('Application')).toBeInTheDocument();
    expect(screen.getByText('About')).toBeInTheDocument();
  });

  it('shows all navigation items', () => {
    render(<SettingsModal opened onClose={noop} />);
    expect(screen.getByText('My Profile')).toBeInTheDocument();
    expect(screen.getByText('Notifications')).toBeInTheDocument();
    expect(screen.getByText('Connections')).toBeInTheDocument();
    expect(screen.getByText('General')).toBeInTheDocument();
    expect(screen.getByText('Import')).toBeInTheDocument();
  });

  it('updates the content header when a nav item is clicked', () => {
    render(<SettingsModal opened onClose={noop} />);
    fireEvent.click(screen.getByText('Notifications'));
    expect(screen.getByRole('heading', { name: 'Notifications' })).toBeInTheDocument();
  });

  it('calls onClose when the close button is clicked', () => {
    const onClose = vi.fn();
    render(<SettingsModal opened onClose={onClose} />);
    fireEvent.click(screen.getByRole('button', { name: /close/i }));
    expect(onClose).toHaveBeenCalledTimes(1);
  });
});
