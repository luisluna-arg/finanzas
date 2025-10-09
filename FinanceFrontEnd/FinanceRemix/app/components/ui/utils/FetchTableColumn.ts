import { InputType } from "./InputType";

export type Row = Record<string, unknown>;

export type Mapper = (record: Row) => unknown;
export type Reducer = (items: Row[]) => unknown;
export type Formatter = (value: unknown) => unknown;

export class FetchTableColumnHeader {
    private class?: string[];
}

export class FetchTableColumnTotals {
    private _formatter?: Formatter;
    private _reducer?: Reducer;

    constructor(formatter?: Formatter, reducer?: Reducer) {
        this._formatter = formatter;
        this._reducer = reducer;
    }

    get formatter(): Formatter | undefined {
        return this._formatter;
    }

    get reducer(): Reducer | undefined {
        return this._reducer;
    }
}

export class FetchTableColumn {
    private _id?: string;
    private _label?: string;
    private _type?: InputType;
    private _class?: string[];
    private _header?: FetchTableColumnHeader;
    private _mapper?: Mapper;
    private _formatter?: Formatter;
    private _totals?: FetchTableColumnTotals;

    constructor(
        id: string,
        label: string,
        mapper?: Mapper,
        totalsReducer?: Reducer
    ) {
        this._id = id;
        this._label = label;
        this._mapper = mapper;
        this._totals = new FetchTableColumnTotals(
            mapper as Formatter | undefined,
            totalsReducer
        );
    }

    get id(): string | undefined {
        return this._id;
    }

    get label(): string | undefined {
        return this._label;
    }
    get mapper(): Mapper | undefined {
        return this._mapper;
    }
    get totals(): FetchTableColumnTotals | undefined {
        return this._totals;
    }
}
