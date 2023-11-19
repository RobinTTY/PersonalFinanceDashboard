// FILEPATH: /home/robin/projects/personal-finance-dashboard/src/client/src/components/modal-button/ModalButton.test.tsx
import { render, fireEvent } from '@test-utils';
import { Icon24Hours } from '@tabler/icons-react';
import { ModalButton } from './ModalButton';
import { ModalButtonProps } from './ModalButtonProps';

describe('ModalButton', () => {
  const mockAction = vi.fn();

  const defaultProps: ModalButtonProps = {
    icon: <Icon24Hours />,
    iconPosition: 'left',
    iconHeight: '50px',
    iconWidth: '50px',
    description: 'Test Description',
    truncateDescription: false,
    textWidth: 100,
    includeChevron: true,
    padding: '10px',
    action: mockAction,
  };

  it('renders without crashing', () => {
    const { container } = render(<ModalButton {...defaultProps} />);
    expect(container.firstChild).toBeInTheDocument();
  });

  it('renders the description', () => {
    const { getByText } = render(<ModalButton {...defaultProps} />);
    expect(getByText(defaultProps.description)).toBeInTheDocument();
  });

  it('calls the action function when clicked', () => {
    const { getByRole } = render(<ModalButton {...defaultProps} />);
    fireEvent.click(getByRole('button'));
    expect(mockAction).toHaveBeenCalled();
  });

  it('renders the chevron when includeChevron is true', () => {
    const { container } = render(<ModalButton {...defaultProps} />);
    expect(container.querySelector('svg.tabler-icon-chevron-right')).toBeInTheDocument();
  });

  it('does not render the chevron when includeChevron is false', () => {
    const { container } = render(<ModalButton {...{ ...defaultProps, includeChevron: false }} />);
    expect(container.querySelector('svg.tabler-icon-chevron-right')).not.toBeInTheDocument();
  });
});
