import { InputType } from "./InputType";

export class FetchTableColumnHeader {
  private class?: string[];
}

export class FetchTableColumnTotals {
  private _formatter?: Function;
  private _reducer?: Function;

  constructor(formatter?: Function, reducer?: Function) {
    this._formatter = formatter;
    this._reducer = reducer;
  }

  get formatter(): Function | undefined {
    return this._formatter;
  }

  get reducer(): Function | undefined {
    return this._reducer;
  }
}

export class FetchTableColumn {
  private _id?: string;
  private _label?: string;
  private _type?: InputType;
  private _class?: string[];
  private _header?: FetchTableColumnHeader;
  private _mapper?: Function;
  private _formatter?: Function;
  private _totals?: FetchTableColumnTotals;

  constructor(id: string, label: string);
  constructor(
    id: string,
    label: string,
    mapper?: Function,
    totalsReducer?: Function
  ) {
    this._id = id;
    this._label = label;
    this._mapper = mapper;
    this._totals = new FetchTableColumnTotals(mapper, totalsReducer);
  }

  get id(): string | undefined {
    return this._id;
  }

  get label(): string | undefined {
    return this._label;
  }
  get mapper(): Function | undefined {
    return this._mapper;
  }
  get totals(): FetchTableColumnTotals | undefined {
    return this._totals;
  }
}
