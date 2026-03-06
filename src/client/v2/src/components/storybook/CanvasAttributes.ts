export interface CanvasAttributes {
  responsive?: boolean;
  dimmed?: boolean;
  canvas: { center: boolean; maxWidth?: number };
  category: string;
  title: string;
  props?: Record<string, any>;
}
