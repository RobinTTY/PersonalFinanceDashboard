import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import "./index.css";

import { ChakraProvider, ColorModeScript } from "@chakra-ui/react";
import theme from "./theme";

const rootElement = document.getElementById("root") as HTMLElement;
ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    {/* <ColorModeScript initialColorMode={theme.config.initialColorMode} /> */}
    {/* <ChakraProvider theme={theme}> */}
    <App />
    {/* </ChakraProvider> */}
  </React.StrictMode>
);
