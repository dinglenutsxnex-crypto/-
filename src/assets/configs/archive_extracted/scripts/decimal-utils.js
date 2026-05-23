//DO NOT CHANGE THIS SETTINGS
Decimal.config({ precision: 20, rounding: Decimal.ROUND_HALF_DOWN });

function decimalAdd(x, y) {
    return new Decimal(x).plus(new Decimal(y)).toString();
}

function decimalSubtract(x, y) {
    return new Decimal(x).sub(new Decimal(y)).toString();
}

function decimalMultiply(x, y) {
    return new Decimal(x).times(new Decimal(y)).toString();
}

function decimalDivide(x, y) {
    return new Decimal(x).dividedBy(new Decimal(y)).toString();
}