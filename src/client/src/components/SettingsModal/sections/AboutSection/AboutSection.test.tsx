import { axe, render } from '@test-utils';
import { screen } from '@testing-library/react';
import { AboutSection } from './AboutSection';

const env = import.meta.env;

describe('AboutSection', () => {
  axe([<AboutSection key="1" />]);

  it('renders the app title', () => {
    render(<AboutSection />);
    expect(screen.getByText(env.VITE_APP_TITLE)).toBeInTheDocument();
  });

  it('renders the version label', () => {
    render(<AboutSection />);
    expect(screen.getByText('Version')).toBeInTheDocument();
  });

  it('renders the description', () => {
    render(<AboutSection />);
    expect(
      screen.getByText(
        'A self-hosted dashboard to track your bank accounts, transactions and stock portfolio.'
      )
    ).toBeInTheDocument();
  });

  it('renders all info row labels', () => {
    render(<AboutSection />);
    expect(screen.getByText('Repository')).toBeInTheDocument();
    expect(screen.getByText('Developer')).toBeInTheDocument();
    expect(screen.getByText('License')).toBeInTheDocument();
    expect(screen.getByText('Report a Bug')).toBeInTheDocument();
  });

  it('renders all link texts', () => {
    render(<AboutSection />);
    expect(screen.getByText('RobinTTY/PersonalFinanceDashboard')).toBeInTheDocument();
    expect(screen.getByText('github.com/RobinTTY')).toBeInTheDocument();
    expect(screen.getByText('View License')).toBeInTheDocument();
    expect(screen.getByText('GitHub Issues')).toBeInTheDocument();
  });

  it('renders all links with correct href attributes', () => {
    render(<AboutSection />);
    expect(screen.getByText('RobinTTY/PersonalFinanceDashboard').closest('a')).toHaveAttribute(
      'href',
      env.VITE_PROJECT_REPOSITORY_URL
    );
    expect(screen.getByText('github.com/RobinTTY').closest('a')).toHaveAttribute(
      'href',
      env.VITE_DEVELOPER_GITHUB_URL
    );
    expect(screen.getByText('View License').closest('a')).toHaveAttribute(
      'href',
      env.VITE_PROJECT_LICENSE_URL
    );
    expect(screen.getByText('GitHub Issues').closest('a')).toHaveAttribute(
      'href',
      env.VITE_PROJECT_ISSUES_URL
    );
  });

  it('renders all links with target="_blank" and rel="noopener noreferrer"', () => {
    render(<AboutSection />);
    const links = screen.getAllByRole('link');
    links.forEach((link) => {
      expect(link).toHaveAttribute('target', '_blank');
      expect(link).toHaveAttribute('rel', 'noopener noreferrer');
    });
  });
});
