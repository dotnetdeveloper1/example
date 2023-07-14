export interface IDictionary<T> {
    add(key: string, value: T): void;
    remove(key: string): void;
    containsKey(key: string): boolean;
    keys(): string[];
    values(): T[];
    get(key: string): T;
}

export class Dictionary<T> implements IDictionary<T> {
    private _keys: string[] = [];
    private _values: T[] = [];

    constructor(init?: { key: string; value: T }[]) {
        if (init) {
            init.forEach((item) => {
                this._keys.push(item.key);
                this._values.push(item.value);
            });
        }
    }

    public add(key: string, value: T): void {
        this._keys.push(key);
        this._values.push(value);
    }

    public remove(key: string): void {
        const index = this._keys.indexOf(key, 0);
        this._keys.splice(index, 1);
        this._values.splice(index, 1);
    }

    public get(key: string): T {
        const index = this._keys.indexOf(key);
        return this._values[index];
    }

    public keys(): string[] {
        return this._keys;
    }

    public values(): T[] {
        return this._values;
    }

    public containsKey(searchingKey: string): boolean {
        return this._keys.some((key) => key === searchingKey);
    }

    public toLookup(): IDictionary<T> {
        return this;
    }
}
