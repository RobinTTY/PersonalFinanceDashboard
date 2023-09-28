// @ts-check
const lightCodeTheme = require("prism-react-renderer/themes/github");
const darkCodeTheme = require("prism-react-renderer/themes/dracula");
const projectTitle = "Personal Finance Dashboard";

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: projectTitle,
  tagline: "Making personal finance simple",
  favicon: "img/favicon.ico",
  url: "https://robintty.github.io",
  baseUrl: "/PersonalFinanceDashboard/",
  onBrokenLinks: "throw",
  onBrokenMarkdownLinks: "warn",
  organizationName: "RobinTTY",
  projectName: "PersonalFinanceDashboard",
  deploymentBranch: "docs",
  trailingSlash: false,
  i18n: {
    defaultLocale: "en",
    locales: ["en"],
  },
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
  plugins: [
    [
      "content-docs",
      {
        id: "dev",
        path: "dev",
        routeBasePath: "dev",
        sidebarPath: require.resolve("./sidebarsDev.js"),
      },
    ],
  ],
  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      image: "img/docusaurus-social-card.jpg",
      navbar: {
        title: "Personal Finance Dashboard",
        logo: {
          alt: "Site Logo",
          src: "img/logo.svg",
        },
        items: [
          {
            type: "doc",
            docId: "getting-started",
            position: "left",
            label: "Docs",
            sidebarId: "docs",
          },
          { to: "/blog", position: "left", label: "Blog" },
          {
            to: "/dev/purpose",
            position: "left",
            label: "Development",
            activeBaseRegex: `dev/`,
            sidebarId: "dev",
          },
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
        copyright: `Copyright © ${new Date().getFullYear()} Robin Müller. Built with Docusaurus.`,
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
      },
    }),
};

module.exports = config;
