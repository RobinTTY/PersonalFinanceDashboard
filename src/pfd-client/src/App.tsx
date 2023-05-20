import { useState } from "react";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import {
  MantineProvider,
  ColorSchemeProvider,
  ColorScheme,
} from "@mantine/core";

import Shell from "./components/shell/Shell";
import ErrorPage from "./routes/ErrorPage";
import Dashboard from "./routes/Dashboard";
import Transactions from "./routes/Transactions";

// TODO: redo with jsx syntax?! See: https://reactrouter.com/en/main/route/route
const router = createBrowserRouter([
  {
    path: "/",
    element: <Shell />,
    errorElement: <ErrorPage />,
    children: [
      {
        index: true,
        element: <Dashboard />,
      },
      {
        path: "dashboard",
        element: <Dashboard />,
      },
      {
        path: "transactions",
        element: <Transactions />,
      },
    ],
  },
]);

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
        <RouterProvider router={router} />
      </MantineProvider>
    </ColorSchemeProvider>
  );
};

export default App;
