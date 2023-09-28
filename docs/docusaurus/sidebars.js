// @ts-check
/** @type {import('@docusaurus/plugin-content-docs').SidebarsConfig} */
const sidebars = {
  docs: [
    "getting-started",
    {
      type: "category",
      label: "Fundamentals",
      items: ["fundamentals/accounts", "fundamentals/transactions"],
    },
  ],
};

module.exports = sidebars;
