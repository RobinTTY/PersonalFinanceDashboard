import classes from './CountryFlag.module.css';

const flagUrls = import.meta.glob<string>('/node_modules/country-flag-icons/3x2/*.svg', {
  query: '?no-inline',
  import: 'default',
  eager: true,
});

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
  const url = flagUrls[`/node_modules/country-flag-icons/3x2/${normalized}.svg`];
  if (!url) {
    return null;
  }

  return <img src={url} alt={normalized} className={classes.flag} />;
}
