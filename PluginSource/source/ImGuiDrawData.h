#pragma once
struct Vector2
{
    float x;
    float y;
};

struct ImVector
{
    int Size;
    int Capacity;
    void* Data;
};

struct ImDrawVert
{
    Vector2 pos;
    Vector2 uv;
    unsigned int col;
};

enum ImDrawListFlags
{
    None = 0,
    AntiAliasedLines = 1 << 0,
    AntiAliasedFill = 1 << 1,
    AllowVtxOffset = 1 << 2,
};

struct ImDrawListSplitter
{
    int _Current;
    int _Count;
    ImVector _Channels;
};

struct ImDrawList
{
    ImVector CmdBuffer;
    ImVector IdxBuffer;
    ImVector VtxBuffer;
    ImDrawListFlags Flags;
    void* _Data;
    char* _OwnerName;
    unsigned int _VtxCurrentOffset;
    unsigned int _VtxCurrentIdx;
    ImDrawVert* _VtxWritePtr;
    unsigned short* _IdxWritePtr;
    ImVector _ClipRectStack;
    ImVector _TextureIdStack;
    ImVector _Path;
    ImDrawListSplitter _Splitter;
};

struct ImDrawData
{
    bool Valid;
    ImDrawList** CmdLists;
    int CmdListsCount;
    int TotalIdxCount;
    int TotalVtxCount;
    Vector2 DisplayPos;
    Vector2 DisplaySize;
    Vector2 FramebufferScale;
};