import "./NivoGridWrapper.css";

// TODO: typing
export const NivoGridWrapper = ({ children }: any) => {
  return (
    <div id="wrapper">
      <div id="child">{children}</div>
    </div>
  );
};
