import { render, fireEvent } from '@test-utils';
import { ContentCheckbox } from './ContentCheckbox';

const mockOnChange = vi.fn();
const defaultProps = {
  checkboxProps: { checked: false },
  onChange: mockOnChange,
};

test('renders without crashing', () => {
  const { container } = render(<ContentCheckbox {...defaultProps}>Test</ContentCheckbox>);
  expect(container.firstChild).toBeInTheDocument();
});

test('renders the children', () => {
  const { getByText } = render(<ContentCheckbox {...defaultProps}>Test</ContentCheckbox>);
  expect(getByText('Test')).toBeInTheDocument();
});

test('calls the onChange function when clicked', () => {
  const { getByRole } = render(<ContentCheckbox {...defaultProps}>Test</ContentCheckbox>);
  fireEvent.click(getByRole('button'));
  expect(mockOnChange).toHaveBeenCalledWith(true);
});

test('checkbox is checked when checkboxProps.checked is true', () => {
  const mockOnChange = vi.fn();

  const props = {
    checkboxProps: { checked: true },
    onChange: mockOnChange,
  };

  const { getByRole } = render(<ContentCheckbox {...props}>Test</ContentCheckbox>);
  expect(getByRole('checkbox')).toBeChecked();
});

test('checkbox is not checked when checkboxProps.checked is false', () => {
  const { getByRole } = render(<ContentCheckbox {...defaultProps}>Test</ContentCheckbox>);
  expect(getByRole('checkbox')).not.toBeChecked();
});
