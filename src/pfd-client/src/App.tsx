import { useState } from "react";
import {
  MantineProvider,
  ColorSchemeProvider,
  ColorScheme,
} from "@mantine/core";
import Shell from "./components/shell/Shell";

const App = () => {
  // TODO: Default color scheme should depend on system settings
  // TODO: Save to localStorage: https://mantine.dev/guides/dark-theme/#save-to-localstorage
  const [colorScheme, setColorScheme] = useState<ColorScheme>("dark");
  const toggleColorScheme = (value?: ColorScheme) =>
    setColorScheme(value || (colorScheme === "dark" ? "light" : "dark"));

  return (
    <ColorSchemeProvider
      colorScheme={colorScheme}
      toggleColorScheme={toggleColorScheme}
    >
      <MantineProvider
        theme={{ colorScheme }}
        withGlobalStyles
        withNormalizeCSS
      >
        <Shell />
      </MantineProvider>
    </ColorSchemeProvider>
  );
};

export default App;
