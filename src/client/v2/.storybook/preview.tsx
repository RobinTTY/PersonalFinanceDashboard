import '@mantine/core/styles.css';

import { ColorSchemeScript, MantineProvider } from '@mantine/core';
import { theme } from '../src/theme';

export const parameters = {
  layout: 'fullscreen',
  options: {
    showPanel: false,
    // Sort stories in the order they are defined in the file, then alphabetically
    storySort: {
      order: ['Accounts', ['AddAccountModal'], 'Settings', ['SettingsModal', 'PreferencesSection'], '*'],
      method: 'alphabetical',
    },
  },
  backgrounds: { disable: true },
};

export const globalTypes = {
  theme: {
    name: 'Theme',
    description: 'Mantine color scheme',
    defaultValue: 'light',
    toolbar: {
      icon: 'mirror',
      items: [
        { value: 'light', title: 'Light' },
        { value: 'dark', title: 'Dark' },
      ],
    },
  },
};

export const decorators = [
  (renderStory: any, context: any) => {
    const scheme = (context.globals.theme || 'light') as 'light' | 'dark';
    return (
      <MantineProvider key={scheme} theme={theme} defaultColorScheme={scheme}>
        <ColorSchemeScript />
        {renderStory()}
      </MantineProvider>
    );
  },
];
