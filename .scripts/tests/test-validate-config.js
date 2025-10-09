const assert = require("assert");
const { checkUrl } = require("../lib/validate-config-lib");

function test() {
    // missing
    let r = checkUrl("X", null, false);
    assert.strictEqual(r.ok, false);

    // invalid
    r = checkUrl("X", "not-a-url", false);
    assert.strictEqual(r.ok, false);

    // http not allowed
    r = checkUrl("X", "http://localhost:5000", false);
    assert.strictEqual(r.ok, false);

    // http allowed when flag true
    r = checkUrl("X", "http://localhost:5000", true);
    assert.strictEqual(r.ok, true);

    // https ok
    r = checkUrl("X", "https://api.example.com", false);
    assert.strictEqual(r.ok, true);

    console.log("validate-config tests passed");
}

test();
