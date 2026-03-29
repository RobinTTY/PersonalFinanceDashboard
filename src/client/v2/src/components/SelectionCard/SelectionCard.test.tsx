import { fireEvent, screen } from '@testing-library/react';
import { axe, render } from '@test-utils';
import { SelectionCard } from './SelectionCard';

const noop = () => {};

describe('SelectionCard', () => {
  axe([<SelectionCard key="1" id="1" optionName="Test Option" isSelected={false} onSelect={noop} />]);

  it('renders the option name', () => {
    render(<SelectionCard id="1" optionName="Savings Account" isSelected={false} onSelect={noop} />);
    expect(screen.getByText('Savings Account')).toBeInTheDocument();
  });

  it('renders initials as fallback when no logoUri is provided', () => {
    render(<SelectionCard id="1" optionName="Savings Account" isSelected={false} onSelect={noop} />);
    expect(screen.getByText('SA')).toBeInTheDocument();
  });

  it('renders single-word initials correctly', () => {
    render(<SelectionCard id="1" optionName="Bitcoin" isSelected={false} onSelect={noop} />);
    expect(screen.getByText('B')).toBeInTheDocument();
  });

  it('renders without selected styling when isSelected is false', () => {
    render(<SelectionCard id="1" optionName="Test Option" isSelected={false} onSelect={noop} />);
    const button = screen.getByRole('button');
    expect(button).not.toHaveAttribute('data-selected');
  });

  it('renders with selected styling when isSelected is true', () => {
    render(<SelectionCard id="1" optionName="Test Option" isSelected onSelect={noop} />);
    const button = screen.getByRole('button');
    expect(button).toHaveAttribute('data-selected');
  });

  it('calls onSelect with the correct id when clicked', () => {
    const onSelect = vi.fn();
    render(<SelectionCard id="bank-123" optionName="Test Option" isSelected={false} onSelect={onSelect} />);
    fireEvent.click(screen.getByRole('button'));
    expect(onSelect).toHaveBeenCalledTimes(1);
    expect(onSelect).toHaveBeenCalledWith('bank-123');
  });

  it('renders an avatar with the provided logoUri as image source', () => {
    render(
      <SelectionCard id="1" optionName="Test Bank" isSelected={false} onSelect={noop} logoUri="https://example.com/logo.png" />
    );
    expect(screen.getByRole('img')).toHaveAttribute('src', 'https://example.com/logo.png');
  });
});
