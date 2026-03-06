import { axe, render } from '@test-utils';
import { screen, fireEvent } from '@testing-library/react';
import { PreferencesSection } from './PreferencesSection';

describe('PreferencesSection', () => {
  axe([<PreferencesSection key="1" />]);

  it('renders the theme setting label and description', () => {
    render(<PreferencesSection />);
    expect(screen.getByText('Theme')).toBeInTheDocument();
    expect(
      screen.getByText('Choose between light, dark, or follow your system setting.')
    ).toBeInTheDocument();
  });

  it('renders all three theme options', () => {
    render(<PreferencesSection />);
    expect(screen.getByText('Light')).toBeInTheDocument();
    expect(screen.getByText('Dark')).toBeInTheDocument();
    expect(screen.getByText('System')).toBeInTheDocument();
  });

  it('selects Light by default', () => {
    render(<PreferencesSection />);
    expect(screen.getByRole('radio', { name: /light/i })).toBeChecked();
  });

  it('switches to Dark when the Dark option is clicked', () => {
    render(<PreferencesSection />);
    fireEvent.click(screen.getByText('Dark'));
    expect(screen.getByRole('radio', { name: /dark/i })).toBeChecked();
  });

  it('switches to System when the System option is clicked', () => {
    render(<PreferencesSection />);
    fireEvent.click(screen.getByText('System'));
    expect(screen.getByRole('radio', { name: /system/i })).toBeChecked();
  });
});
