import React from "react";
import { IconCheck } from "@tabler/icons-react";

import classes from "./HeroHeader.module.css";

export const HeroHeader = () => {
  return (
    <div className={classes["header-container"]}>
      <div className={classes.title}>
        The <span className={classes.highlight}>modern</span> way of managing
        your finances
      </div>
      <div className={classes.description}>
        Visualize your personal finances, with tools that help analyze spending
        habits and performance of investments. Automates the import of bank
        statements and stock performance through utilization of bank/stock APIs.
      </div>
      <ul className={classes.features}>
        <li className={classes.feature}>
          <span className={classes["icon-container"]}>
            <IconCheck className={classes.icon} />
          </span>
          <b>Data import</b> – Import transactions from your bank automatically,
          no need for manual entry
        </li>
        <li className={classes.feature}>
          <span className={classes["icon-container"]}>
            <IconCheck className={classes.icon} />
          </span>
          <b>Analytics</b> – Get an overview of your finances through easy to
          understand reports
        </li>
        <li className={classes.feature}>
          <span className={classes["icon-container"]}>
            <IconCheck className={classes.icon} />
          </span>
          <b>Local Storage</b> – All your data remains on your device, safe and
          sound
        </li>
      </ul>
    </div>
  );
};
