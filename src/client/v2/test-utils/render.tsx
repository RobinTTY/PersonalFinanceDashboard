import { MockedProvider } from '@apollo/client/testing/react';
import { act, RenderResult, render as testingLibraryRender } from '@testing-library/react';
import { MantineProvider, MantineProviderProps, MantineThemeOverride } from '@mantine/core';

export function render(
  ui: React.ReactNode,
  themeOverride?: MantineThemeOverride,
  providerProps?: Omit<MantineProviderProps, 'theme'>
) {
  return testingLibraryRender(ui, {
    wrapper: ({ children }: { children: React.ReactNode }) => (
      <MockedProvider>
        <MantineProvider theme={themeOverride} env="test" {...providerProps}>
          {children}
        </MantineProvider>
      </MockedProvider>
    ),
  });
}

export async function renderWithAct(ui: React.ReactNode) {
  let result: RenderResult | null = null;
  await act(async () => {
    result = render(ui);
  });
  return result!;
}
