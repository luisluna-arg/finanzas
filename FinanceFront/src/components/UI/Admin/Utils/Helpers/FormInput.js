export default class FormInput {
  constructor(id, type, label, placeholder = null, visible = true) {
    this.id = id;
    this.type = type;
    this.label = label;
    this.placeholder = placeholder;
    this.visible = visible;
  }
}
