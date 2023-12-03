// MyClass.js
export default class FormInput {
  constructor(id, type, label, placeholder = null) {
    this._id = id;
    this._type = type;
    this._label = label;
    this._placeholder = placeholder;
  }

  get id() {
    return this._value;
  }

  set id(value) {
    this._id = value;
  }

  get type() {
    return this._value;
  }

  set type(value) {
    this._type = value;
  }

  get label() {
    return this._value;
  }

  set label(value) {
    this._label = value;
  }

  get placeholder() {
    return this._value;
  }

  set placeholder(value) {
    this._placeholder = value;
  }
}
