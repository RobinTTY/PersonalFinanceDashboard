/**
 * TODO: Add description
 * @param locale
 * @param dateStyle
 * @param timeStyle
 * @param timeZone
 */
export const getDateFormatter = (
  locale: string,
  dateStyle?: Intl.DateTimeFormatOptions['dateStyle'],
  timeStyle?: Intl.DateTimeFormatOptions['timeStyle'],
  timeZone?: Intl.DateTimeFormatOptions['timeZone']
) =>
  new Intl.DateTimeFormat(locale, {
    dateStyle,
    timeStyle,
    timeZone,
  });
