import { useState } from "react";
import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider,
} from "react-router-dom";
import {
  MantineProvider,
  ColorSchemeProvider,
  ColorScheme,
} from "@mantine/core";
import { ApolloProvider, ApolloClient, InMemoryCache } from "@apollo/client";

import Shell from "./components/shell/Shell";
import ErrorPage from "./routes/ErrorPage";
import Dashboard from "./routes/Dashboard";
import Transactions from "./routes/Transactions";

// Appolo (GraphQL) Setup
const client = new ApolloClient({
  uri: "https://localhost:7115",
  cache: new InMemoryCache(),
});

// React Router Setup
const router = createBrowserRouter(
  createRoutesFromElements(
    <Route
      path="/"
      element={<Shell />}
      // loader={rootLoader}
      // action={rootAction}
      errorElement={<ErrorPage />}
    >
      {/* <Route errorElement={<ErrorPage />}> */}
      <Route index element={<Dashboard />} />
      <Route path="dashboard" element={<Dashboard />} />
      <Route path="transactions" element={<Transactions />} />
      {/* </Route> */}
    </Route>
  )
);

const App = () => {
  // TODO: Default color scheme should depend on system settings
  // TODO: Save to localStorage: https://mantine.dev/guides/dark-theme/#save-to-localstorage
  const [colorScheme, setColorScheme] = useState<ColorScheme>("dark");
  const toggleColorScheme = (value?: ColorScheme) =>
    setColorScheme(value || (colorScheme === "dark" ? "light" : "dark"));

  return (
    <ApolloProvider client={client}>
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
    </ApolloProvider>
  );
};

export default App;
