import { createContext, useContext } from 'react';

/**
 * Opens the settings modal, optionally at the section with the given label (e.g. "Connected Banks").
 */
export type OpenSettings = (section?: string) => void;

const OpenSettingsContext = createContext<OpenSettings | null>(null);

export const OpenSettingsProvider = OpenSettingsContext.Provider;

/**
 * Gets the function that opens the settings modal, or `null` outside the app layout (e.g. in stories).
 */
export function useOpenSettings() {
  return useContext(OpenSettingsContext);
}
