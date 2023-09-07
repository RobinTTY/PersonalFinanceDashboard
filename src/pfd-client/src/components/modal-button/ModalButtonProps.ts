import { SystemProp } from "@mantine/core";
import { CSSProperties } from "react";

export interface ModalButtonProps {
  icon: JSX.Element;
  iconPosition?: "left" | "right" | "top" | "bottom";
  iconHeight?: SystemProp<CSSProperties["height"]>;
  description: string;
  action: () => void;
}
