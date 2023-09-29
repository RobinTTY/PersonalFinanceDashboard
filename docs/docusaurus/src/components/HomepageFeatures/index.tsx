import clsx from "clsx";
import Heading from "@theme/Heading";
import classes from "./styles.module.css";
import useDocusaurusContext from "@docusaurus/useDocusaurusContext";

type FeatureItem = {
  title: string;
  Svg: React.ComponentType<React.ComponentProps<"svg">>;
  description: JSX.Element;
};

function Feature({ title, Svg, description }: FeatureItem) {
  const containerStyles = "text--center padding-horiz--md " + classes.container;
  return (
    <div className={clsx("col col--4")}>
      <div className="text--center">
        <Svg className={classes.featureSvg} role="img" />
      </div>
      <div className={containerStyles}>
        <Heading as="h3">{title}</Heading>
        <p className={classes.description}>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): JSX.Element {
  const { siteConfig } = useDocusaurusContext();

  const FeatureList: FeatureItem[] = [
    {
      title: "Easy to Use",
      Svg: require("@site/static/img/undraw_docusaurus_mountain.svg").default,
      description: (
        <>
          {siteConfig.title} was designed from the ground up to be easy to use
          and give you control over your finances.
        </>
      ),
    },
    {
      title: "Focus on What Matters",
      Svg: require("@site/static/img/undraw_docusaurus_tree.svg").default,
      description: (
        <>
          {siteConfig.title} lets you focus on your finances, and we&apos;ll do
          the chores. Go ahead and move your money around, we&apos;ll keep track
          of it.
        </>
      ),
    },
    {
      title: "Set your own goals",
      Svg: require("@site/static/img/undraw_docusaurus_react.svg").default,
      description: (
        <>
          Set saving goals and track your progress. {siteConfig.title} will help
          you keep track of your progress and provide helpful analytics along
          the way.
        </>
      ),
    },
  ];

  return (
    <section className={classes.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
