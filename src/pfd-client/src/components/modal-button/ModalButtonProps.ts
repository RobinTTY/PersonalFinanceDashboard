export interface ModalButtonProps {
  icon: JSX.Element;
  iconPosition?: "left" | "right" | "top" | "bottom";
  iconSize?: "xs" | "sm" | "md" | "lg" | "xl";
  description: string;
  action: () => void;
}
