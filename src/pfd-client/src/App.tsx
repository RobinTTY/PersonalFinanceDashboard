import { MantineProvider, Text } from "@mantine/core";
import Shell from "./components/pfd-shell/Shell";

const App = () => {
  return (
    <MantineProvider withGlobalStyles withNormalizeCSS>
      <Shell />
    </MantineProvider>
  );
};

export default App;
