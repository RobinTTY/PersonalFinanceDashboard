import "./NivoGridWrapper.css";

// TODO: typing
export const NivoGridWrapper: React.FC<React.PropsWithChildren> = ({
  children,
}: any) => {
  console.log(children);
  return (
    <div id="wrapper">
      <div id="child">{children}</div>
    </div>
  );
};
