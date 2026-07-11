import { hasFlag } from 'country-flag-icons';
import * as Flags from 'country-flag-icons/react/3x2';
import classes from './CountryFlag.module.css';

type FlagComponent = (props: React.SVGAttributes<SVGElement>) => React.JSX.Element;

interface CountryFlagProps {
  /** ISO 3166-1 alpha-2 country code (case-insensitive), e.g. "SE". */
  code: string;
}

/**
 * Renders the SVG flag for the given ISO 3166-1 alpha-2 country code.
 * Returns `null` for codes without an available flag so callers don't have to guard.
 */
export function CountryFlag({ code }: CountryFlagProps) {
  const normalized = code.toUpperCase();
  if (!hasFlag(normalized)) {
    return null;
  }

  const Flag = (Flags as Record<string, FlagComponent>)[normalized];
  return <Flag className={classes.flag} aria-label={normalized} role="img" />;
}
