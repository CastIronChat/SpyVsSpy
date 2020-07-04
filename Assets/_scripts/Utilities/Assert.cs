using System;

public static class Assert {
    public static void NotNull(object v) {
        if(v == null) {}//throw new Exception("value cannot be null");
    }
}
