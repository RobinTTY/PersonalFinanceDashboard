// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require("prism-react-renderer/themes/github");
const darkCodeTheme = require("prism-react-renderer/themes/dracula");

const projectTitle = "Personal Finance Dashboard";

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: projectTitle,
  tagline: "Making personal finance simple.",
  url: "https://robintty.github.io",
  baseUrl: "/PersonalFinanceDashboard/",
  onBrokenLinks: "throw",
  onBrokenMarkdownLinks: "warn",
  favicon: "img/favicon.ico",
  organizationName: "RobinTTY",
  projectName: "PersonalFinanceDashboard",
  deploymentBranch: "docs",
  trailingSlash: false,
  presets: [
    [
      "classic",
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve("./sidebars.js"),
        },
        blog: {
          showReadingTime: true,
        },
        theme: {
          customCss: require.resolve("./src/css/custom.css"),
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      navbar: {
        title: projectTitle,
        logo: {
          alt: "My Site Logo",
          src: "img/logo.svg",
        },
        items: [
          {
            type: "doc",
            docId: "getting-started",
            position: "left",
            label: "Docs",
          },
          { to: "/blog", label: "Blog", position: "left" },
        ],
      },
      footer: {
        style: "dark",
        links: [
          {
            title: "Learn",
            items: [
              {
                label: "Docs",
                to: "/docs/getting-started",
              },
            ],
          },
          {
            title: "Socials",
            items: [
              {
                label: "Twitter",
                href: "https://twitter.com/Robin_tty",
              },
              {
                label: "LinkedIn",
                href: "https://www.linkedin.com/in/robin-m%C3%BCller-574782170",
              },
            ],
          },
          {
            title: "More",
            items: [
              {
                label: "Blog",
                to: "/blog",
              },
              {
                label: "GitHub",
                href: "https://github.com/RobinTTY/PersonalFinanceDashboard",
              },
            ],
          },
        ],
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
      },
    }),
};

module.exports = config;
