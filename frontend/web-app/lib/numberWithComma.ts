//  formats a number as a string with commas as thousand separators
export function numberWithCommas(amount: number) {
  return amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}
