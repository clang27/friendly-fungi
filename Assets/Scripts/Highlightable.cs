/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

public interface Highlightable {
    protected QuickOutline Outline { get; }
    public void Highlight(bool b) {
        Outline.enabled = b;
    }

    public void Click();
}
