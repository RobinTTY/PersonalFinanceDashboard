import type { StorybookConfig } from '@storybook/react-vite';

const config: StorybookConfig = {
  stories: ['../src/**/*.mdx', '../src/**/*.story.@(js|jsx|ts|tsx)'],
  addons: ['@storybook/addon-styling', '@storybook/addon-docs', '@storybook/addon-themes'],

  framework: {
    name: '@storybook/react-vite',
    options: {},
  }
};

export default config;
